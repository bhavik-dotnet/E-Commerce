import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { Cart, AddToCartRequest, UpdateCartItemRequest } from '../models/cart.model';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/cart`;
  
  private cartSubject = new BehaviorSubject<Cart>({ items: [], totalAmount: 0, totalItems: 0 });
  public cart$ = this.cartSubject.asObservable();

  get cartValue(): Cart {
    return this.cartSubject.value;
  }

  getCart(): Observable<Cart> {
    return this.http.get<Cart>(this.apiUrl).pipe(
      tap(cart => this.cartSubject.next(cart))
    );
  }

  addToCart(request: AddToCartRequest): Observable<Cart> {
    return this.http.post<Cart>(this.apiUrl, request).pipe(
      tap(cart => this.cartSubject.next(cart))
    );
  }

  updateCartItem(cartItemId: number, request: UpdateCartItemRequest): Observable<Cart> {
    return this.http.put<Cart>(`${this.apiUrl}/${cartItemId}`, request).pipe(
      tap(cart => this.cartSubject.next(cart))
    );
  }

  removeFromCart(cartItemId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${cartItemId}`).pipe(
      tap(() => this.getCart().subscribe())
    );
  }

  clearCart(): Observable<any> {
    return this.http.delete(this.apiUrl).pipe(
      tap(() => this.cartSubject.next({ items: [], totalAmount: 0, totalItems: 0 }))
    );
  }
}