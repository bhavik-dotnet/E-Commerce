import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CartService } from '../../../services/cart.service';
import { OrderService } from '../../../services/order.service';
import { AuthService } from '../../../services/auth.service';
import { Cart, CartItem } from '../../../models/cart.model';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {
  private cartService = inject(CartService);
  private orderService = inject(OrderService);
  private authService = inject(AuthService);
  private router = inject(Router);

  cart: Cart = { items: [], totalAmount: 0, totalItems: 0 };
  loading = false;
  processingPayment = false;
  successMessage = '';
  errorMessage = '';

  ngOnInit(): void {
    this.loadCart();
  }

  loadCart(): void {
    this.loading = true;
    this.cartService.getCart().subscribe({
      next: (cart) => {
        this.cart = cart;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load cart';
        this.loading = false;
      }
    });
  }

  updateQuantity(cartItem: CartItem, quantity: number): void {
    if (quantity < 1 || quantity > 10 || quantity > cartItem.availableQuantity) {
      return;
    }

    this.cartService.updateCartItem(cartItem.cartItemId, { quantity }).subscribe({
      next: (cart) => {
        this.cart = cart;
        this.successMessage = 'Cart updated';
        setTimeout(() => this.successMessage = '', 2000);
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to update cart';
        setTimeout(() => this.errorMessage = '', 3000);
      }
    });
  }

  removeItem(cartItemId: number): void {
    if (confirm('Remove this item from cart?')) {
      this.cartService.removeFromCart(cartItemId).subscribe({
        next: () => {
          this.loadCart();
          this.successMessage = 'Item removed from cart';
          setTimeout(() => this.successMessage = '', 2000);
        },
        error: () => {
          this.errorMessage = 'Failed to remove item';
          setTimeout(() => this.errorMessage = '', 3000);
        }
      });
    }
  }

  processPayment(): void {
    if (this.cart.items.length === 0) {
      return;
    }

    this.processingPayment = true;
    this.orderService.createOrder({ totalAmount: this.cart.totalAmount }).subscribe({
      next: () => {
        this.showPaymentSuccess();
        this.loadCart();
        this.processingPayment = false;
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Payment failed';
        this.processingPayment = false;
        setTimeout(() => this.errorMessage = '', 3000);
      }
    });
  }

  showPaymentSuccess(): void {
    this.successMessage = 'Payment successful! Order placed.';
    setTimeout(() => {
      this.successMessage = '';
      this.router.navigate(['/customer/orders']);
    }, 2000);
  }

  continueShopping(): void {
    this.router.navigate(['/customer/products']);
  }

  goToOrders(): void {
    this.router.navigate(['/customer/orders']);
  }

  logout(): void {
    this.authService.logout();
  }

  getQuantityOptions(max: number): number[] {
    const limit = Math.min(max, 10);
    return Array.from({ length: limit }, (_, i) => i + 1);
  }
}