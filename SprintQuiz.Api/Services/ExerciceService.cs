using AutoMapper;
using SprintQuiz.Api.Data;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace SprintQuiz.Api.Services
{
    public class ExerciceService : IExerciceService
    {
        private readonly SprintQuizDbContext _context;
        private readonly IMapper _mapper;

        public ExerciceService(SprintQuizDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // --- CRUD Exercices ---
        public async Task<IEnumerable<ExerciceDto>> GetAllExercicesAsync()
        {
            var exercices = await _context.Exercices
                .Include(e => e.Indices)
                .Include(e => e.EtapesResolution)
                .OrderBy(e => e.DateCreation)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ExerciceDto>>(exercices);
        }

        public async Task<ExerciceDto?> GetExerciceByIdAsync(Guid id, Guid? utilisateurId = null)
        {
            var exercice = await _context.Exercices.FindAsync(id);
            if (exercice == null) return null;

            var dto = _mapper.Map<ExerciceDto>(exercice);

            if (utilisateurId.HasValue)
            {
                var progression = await _context.ProgressionsUtilisateur
                    .FirstOrDefaultAsync(p => p.UtilisateurId == utilisateurId.Value
                                           && p.Niveau == NiveauEnum.Exercice
                                           && p.NiveauId == id);
                dto.DerniereActivite = progression?.DerniereActivite;
            }

            return dto;
        }

        public async Task<IEnumerable<ExerciceDto>> GetExercicesByNiveauAsync(NiveauEnum niveau, Guid niveauId)
        {
            var exercices = await _context.Exercices
                .Include(e => e.Indices)
                .Include(e => e.EtapesResolution)
                .Where(e => e.Niveau == niveau && e.NiveauId == niveauId)
                .OrderBy(e => e.DateCreation)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ExerciceDto>>(exercices);
        }

        public async Task<ExerciceDto> CreateExerciceAsync(CreateExerciceDto createDto)
        {
            var exercice = _mapper.Map<Exercice>(createDto);
            exercice.Id = Guid.NewGuid();

            // Gérer les indices
            if (createDto.Indices != null && createDto.Indices.Any())
            {
                exercice.Indices = createDto.Indices.Select(i => new Indice
                {
                    Id = Guid.NewGuid(),
                    ExerciceId = exercice.Id,
                    Ordre = i.Ordre,
                    Texte = i.Texte
                }).ToList();
            }

            // Gérer les étapes
            if (createDto.EtapesResolution != null && createDto.EtapesResolution.Any())
            {
                exercice.EtapesResolution = createDto.EtapesResolution.Select(e => new EtapeResolution
                {
                    Id = Guid.NewGuid(),
                    ExerciceId = exercice.Id,
                    Ordre = e.Ordre,
                    Description = e.Description
                }).ToList();
            }

            _context.Exercices.Add(exercice);
            await _context.SaveChangesAsync();
            return _mapper.Map<ExerciceDto>(exercice);
        }

        public async Task<ExerciceDto?> UpdateExerciceAsync(Guid id, UpdateExerciceDto updateDto)
        {
            var exercice = await _context.Exercices.FindAsync(id);
            if (exercice == null) return null;

            _mapper.Map(updateDto, exercice);
            exercice.DerniereModification = DateTime.UtcNow;
            exercice.DureeEstimee = CalculateEstimatedTimeForExercice(exercice);

            _context.Exercices.Update(exercice);
            await _context.SaveChangesAsync();
            return _mapper.Map<ExerciceDto>(exercice);
        }

        public async Task<bool> DeleteExerciceAsync(Guid id)
        {
            var exercice = await _context.Exercices.FindAsync(id);
            if (exercice == null) return false;

            _context.Exercices.Remove(exercice);
            await _context.SaveChangesAsync();
            return true;
        }

        // --- Consultation ---
        public async Task<ConsultationExerciceDto> ConsultExerciceAsync(Guid utilisateurId, CreateConsultationExerciceDto consultationDto)
        {
            var consultation = new ConsultationExercice
            {
                Id = Guid.NewGuid(),
                UtilisateurId = utilisateurId,
                ExerciceId = consultationDto.ExerciceId,
                DateConsultation = DateTime.UtcNow,
                MarqueeCompris = consultationDto.MarqueeCompris,
                AConsulteSolution = false,
                AUtiliseIndices = false,
                TentativesAvantSolution = 0
            };

            _context.ConsultationsExercice.Add(consultation);
            await _context.SaveChangesAsync();

            await UpdateUserProgressionAsync(utilisateurId, consultationDto.ExerciceId);

            return _mapper.Map<ConsultationExerciceDto>(consultation);
        }

        public async Task<IEnumerable<ConsultationExerciceDto>> GetUserConsultationsAsync(Guid utilisateurId)
        {
            var consultations = await _context.ConsultationsExercice
                .Where(c => c.UtilisateurId == utilisateurId)
                .OrderByDescending(c => c.DateConsultation)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ConsultationExerciceDto>>(consultations);
        }

        public async Task<IEnumerable<ExerciceDto>> GetExercicesForRevisionAsync(Guid utilisateurId, NiveauEnum niveau, Guid niveauId)
        {
            var exercices = await _context.Exercices
                .Include(e => e.Indices)
                .Include(e => e.EtapesResolution)
                .Where(e => e.Niveau == niveau && e.NiveauId == niveauId)
                .ToListAsync();

            var consultedIds = await _context.ConsultationsExercice
                .Where(c => c.UtilisateurId == utilisateurId)
                .Select(c => c.ExerciceId)
                .ToListAsync();

            var exercicesForRevision = exercices
                .OrderBy(e => consultedIds.Contains(e.Id) ? 1 : 0)
                .ThenBy(e => e.NiveauDifficulte)
                .ToList();

            return _mapper.Map<IEnumerable<ExerciceDto>>(exercicesForRevision);
        }

        private async Task UpdateUserProgressionAsync(Guid utilisateurId, Guid exerciceId)
        {
            var exercice = await _context.Exercices.FindAsync(exerciceId);
            if (exercice == null) return;

            var progression = await _context.ProgressionsUtilisateur
                .FirstOrDefaultAsync(p => p.UtilisateurId == utilisateurId &&
                                          p.Niveau == exercice.Niveau &&
                                          p.NiveauId == exercice.NiveauId);

            if (progression == null)
            {
                progression = new ProgressionUtilisateur
                {
                    Id = Guid.NewGuid(),
                    UtilisateurId = utilisateurId,
                    Niveau = exercice.Niveau,
                    NiveauId = exercice.NiveauId,
                    PourcentageComplet = 0,
                    DerniereActivite = DateTime.UtcNow
                };
                _context.ProgressionsUtilisateur.Add(progression);
            }

            var total = await _context.Exercices
                .CountAsync(e => e.Niveau == exercice.Niveau && e.NiveauId == exercice.NiveauId);

            var consulted = await _context.ConsultationsExercice
                .Where(c => c.UtilisateurId == utilisateurId)
                .Join(_context.Exercices,
                      c => c.ExerciceId,
                      e => e.Id,
                      (c, e) => new { c, e })
                .Where(ce => ce.e.Niveau == exercice.Niveau && ce.e.NiveauId == exercice.NiveauId)
                .CountAsync();

            progression.PourcentageComplet = total > 0 ? (float)consulted / total : 0;
            progression.DerniereActivite = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // --- Gestion des Indices ---
        public async Task<IEnumerable<IndiceDto>> GetIndicesByExerciceIdAsync(Guid exerciceId)
        {
            var indices = await _context.Indices
                .Where(i => i.ExerciceId == exerciceId)
                .OrderBy(i => i.Ordre)
                .ToListAsync();
            return _mapper.Map<IEnumerable<IndiceDto>>(indices);
        }

        public async Task<IndiceDto?> GetIndiceByIdAsync(Guid id)
        {
            var indice = await _context.Indices.FindAsync(id);
            return indice == null ? null : _mapper.Map<IndiceDto>(indice);
        }

        public async Task<IndiceDto> CreateIndiceAsync(CreateIndiceDto createDto)
        {
            var indice = _mapper.Map<Indice>(createDto);
            indice.Id = Guid.NewGuid();
            _context.Indices.Add(indice);
            await _context.SaveChangesAsync();
            return _mapper.Map<IndiceDto>(indice);
        }

        public async Task<IndiceDto?> UpdateIndiceAsync(Guid id, UpdateIndiceDto updateDto)
        {
            var indice = await _context.Indices.FindAsync(id);
            if (indice == null) return null;
            _mapper.Map(updateDto, indice);
            await _context.SaveChangesAsync();
            return _mapper.Map<IndiceDto>(indice);
        }

        public async Task<bool> DeleteIndiceAsync(Guid id)
        {
            var indice = await _context.Indices.FindAsync(id);
            if (indice == null) return false;
            _context.Indices.Remove(indice);
            await _context.SaveChangesAsync();
            return true;
        }

        // --- Gestion des Étapes de Résolution ---
        public async Task<IEnumerable<EtapeResolutionDto>> GetEtapesByExerciceIdAsync(Guid exerciceId)
        {
            var etapes = await _context.EtapesResolution
                .Where(e => e.ExerciceId == exerciceId)
                .OrderBy(e => e.Ordre)
                .ToListAsync();
            return _mapper.Map<IEnumerable<EtapeResolutionDto>>(etapes);
        }

        public async Task<EtapeResolutionDto?> GetEtapeByIdAsync(Guid id)
        {
            var etape = await _context.EtapesResolution.FindAsync(id);
            return etape == null ? null : _mapper.Map<EtapeResolutionDto>(etape);
        }

        public async Task<EtapeResolutionDto> CreateEtapeAsync(CreateEtapeResolutionDto createDto)
        {
            var etape = _mapper.Map<EtapeResolution>(createDto);
            etape.Id = Guid.NewGuid();
            _context.EtapesResolution.Add(etape);
            await _context.SaveChangesAsync();
            return _mapper.Map<EtapeResolutionDto>(etape);
        }

        public async Task<EtapeResolutionDto?> UpdateEtapeAsync(Guid id, UpdateEtapeResolutionDto updateDto)
        {
            var etape = await _context.EtapesResolution.FindAsync(id);
            if (etape == null) return null;
            _mapper.Map(updateDto, etape);
            await _context.SaveChangesAsync();
            return _mapper.Map<EtapeResolutionDto>(etape);
        }

        public async Task<bool> DeleteEtapeAsync(Guid id)
        {
            var etape = await _context.EtapesResolution.FindAsync(id);
            if (etape == null) return false;
            _context.EtapesResolution.Remove(etape);
            await _context.SaveChangesAsync();
            return true;
        }

        private int CalculateEstimatedTimeForExercice(Exercice exercice)
        {
            return exercice.Type switch
            {
                TypeExercice.Basique => 2,
                TypeExercice.Applique => 4,
                TypeExercice.Analyse or TypeExercice.Cas => 7,
                TypeExercice.Defi => 10,
                _ => 2
            };
        }
    }
}
