import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router'; 
import { AuthLayoutComponent } from '../../shared/layouts/auth-layout/auth-layout.component';
import { AuthService } from '../../core/services/auth.service';
import { LoginDto } from '../../core/models/auth.model';
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, AuthLayoutComponent],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  showPassword = false;
  isLoading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private authService: AuthService
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      motDePasse: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngOnInit(): void {
    // Si déjà connecté, rediriger
    if (this.authService.isAuthenticated()) {
      this.redirectUser();
    }
  }

  onSubmit(): void {
    if (this.loginForm.invalid) return;

    this.isLoading = true;
    this.errorMessage = '';

    const loginData: LoginDto = this.loginForm.value;

    this.authService.login(loginData).subscribe({
      next: (response) => {
        this.isLoading = false;
        console.log('Connexion réussie', response);
        this.redirectUser();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = this.getErrorMessage(err);
        this.shakeForm();
      }
    });
  }

  private getErrorMessage(err: any): string {
    if (err.status === 401) {
      return 'Email ou mot de passe incorrect.';
    }
    if (err.status === 404) {
      return 'Utilisateur non trouvé.';
    }
    return 'Une erreur est survenue. Veuillez réessayer.';
  }

  private shakeForm(): void {
    const container = document.querySelector('.login-container');
    if (container) {
      container.classList.add('shake');
      setTimeout(() => container.classList.remove('shake'), 500);
    }
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  onForgotPassword(): void {
    this.router.navigate(['/auth/forgot-password']);
  }

  private redirectUser(): void {
    const user = this.authService.getCurrentUser();
    if (user?.role === 0) {
      this.router.navigate(['/admin/dashboard']);
    } else {
      this.router.navigate(['/user/dashboard']);
    }
  }
}