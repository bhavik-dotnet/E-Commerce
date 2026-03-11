import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { OrderService } from '../../../services/order.service';
import { AuthService } from '../../../services/auth.service';
import { Order } from '../../../models/order.model';

@Component({
  selector: 'app-order-history',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './order-history.component.html',
  styleUrls: ['./order-history.component.css']
})
export class OrderHistoryComponent implements OnInit {
  private orderService = inject(OrderService);
  private authService = inject(AuthService);
  private router = inject(Router);

  orders: Order[] = [];
  loading = false;
  errorMessage = '';
  expandedOrderId: number | null = null;

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
  this.loading = true;
  this.orderService.getOrderHistory().subscribe({
    next: (orders) => {
      console.log("📦 Raw orders from API:", orders);

      this.orders = orders.map(order => {
        console.log(`➡️ Processing order #${order.orderId}`, order.items);

        return {
          ...order,
          items: Array.isArray(order.items) ? order.items : []
        };
      });

      console.log("✅ Final mapped orders:", this.orders);
      this.loading = false;
    },
    error: (err) => {
      console.error("❌ Failed to load order history", err);
      this.errorMessage = 'Failed to load order history';
      this.loading = false;
    }
  });
}


  toggleOrderDetails(orderId: number): void {
    this.expandedOrderId = this.expandedOrderId === orderId ? null : orderId;
  }

  isExpanded(orderId: number): boolean {
    return this.expandedOrderId === orderId;
  }

  continueShopping(): void {
    this.router.navigate(['/customer/products']);
  }

  goToCart(): void {
    this.router.navigate(['/customer/cart']);
  }

  logout(): void {
    this.authService.logout();
  }
}
