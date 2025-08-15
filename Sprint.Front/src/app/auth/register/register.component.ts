import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';  
import { AuthService } from '../../core/services/auth.service';
import { AuthLayoutComponent } from '../../shared/layouts/auth-layout/auth-layout.component';
import { RoleUtilisateur } from '../../core/models/enums.models';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, AuthLayoutComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  roles = RoleUtilisateur;
  errorMessage = '';
  isLoading = false;

  showPassword = false;
showConfirmPassword = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      nom: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      motDePasse: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
      role: [RoleUtilisateur.Etudiant, [Validators.required]]
    }, {
      validators: this.passwordMatchValidator
    });
  }

  ngOnInit(): void {
    // Si l'utilisateur n'est pas admin, rediriger
    // if (!this.authService.isAdmin()) {
    //   this.router.navigate(['/admin/dashboard']);
    // }
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('motDePasse')?.value;
    const confirmPassword = form.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { mismatch: true };
  }

  onSubmit() {
    if (this.registerForm.invalid) {
      this.errorMessage = 'Veuillez remplir tous les champs correctement.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const { nom, email, motDePasse, role } = this.registerForm.value;

    const userData = { nom, email, motDePasse, role };

    this.authService.createUser(userData).subscribe({
      next: (user) => {
        this.isLoading = false;
        alert(`Utilisateur ${user.nom} créé avec succès !`);
        this.router.navigate(['/admin/utilisateurs']);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Erreur lors de la création de l’utilisateur.';
      }
    });
  }

  togglePassword() {
  this.showPassword = !this.showPassword;
}

toggleConfirmPassword() {
  this.showConfirmPassword = !this.showConfirmPassword;
}

  onCancel() {
    this.router.navigate(['/admin/utilisateurs']);
  }
}
