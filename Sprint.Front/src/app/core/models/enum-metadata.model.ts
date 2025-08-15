// src/app/core/models/enum-metadata.model.ts

export interface EnumMetadata {
  label: string;
  color: string;
  icon: string;
  class: string;
}

export interface EnumConfig {
  NiveauEnum: Record<number, EnumMetadata>;
  NiveauDifficulte: Record<number, EnumMetadata>;
  RoleUtilisateur: Record<number, EnumMetadata>;
  TypeExercice: Record<number, EnumMetadata>;
}