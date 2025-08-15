import { HttpInterceptorFn } from '@angular/common/http';




export const authInterceptor: HttpInterceptorFn = (req, next) => {
  
  const PUBLIC_ENDPOINTS = [
    '/api/utilisateur/login',
    '/api/utilisateur',
    '/api/utilisateur/forgot-password'
  ];

  const isPublic = PUBLIC_ENDPOINTS.some(endpoint => req.url.includes(endpoint));
  if (isPublic) {
    console.log('Skipping authentication for public endpoint');
    return next(req);
  }

  const token = localStorage.getItem('jwt_token'); 

  if (token) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    }); 
  } else {
    console.warn('No JWT token found in localStorage');
  }

  return next(req);
};
