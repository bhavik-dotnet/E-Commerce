import { Routes } from '@angular/router';
import { authGuard, adminGuard } from './guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    loadComponent: () => import('./components/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./components/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'admin',
    canActivate: [authGuard, adminGuard],
    loadComponent: () => import('./components/admin/admin.component').then(m => m.AdminComponent)
  },
  {
    path: 'customer',
    canActivate: [authGuard],
    children: [
      {
        path: 'products',
        loadComponent: () => import('./components/customer/product-list/product-list.component').then(m => m.ProductListComponent)
      },
      {
        path: 'cart',
        loadComponent: () => import('./components/customer/cart/cart.component').then(m => m.CartComponent)
      },
      {
        path: 'orders',
        loadComponent: () => import('./components/customer/order-history/order-history.component').then(m => m.OrderHistoryComponent)
      }
    ]
  },
  {
    path: '**',
    redirectTo: '/login'
  }
];