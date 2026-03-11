import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Product, ProductDetail, CreateProductRequest, UpdateProductRequest, Category, ProductFilter, PagedProductResponse } from '../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/products`;
  private categoriesUrl = `${environment.apiUrl}/categories`;

  getAllProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(this.apiUrl);
  }

  getFilteredProducts(filter: ProductFilter): Observable<PagedProductResponse> {
    return this.http.post<PagedProductResponse>(`${this.apiUrl}/filter`, filter);
  }

  getProductById(id: number): Observable<ProductDetail> {
    return this.http.get<ProductDetail>(`${this.apiUrl}/${id}`);
  }

  createProduct(product: CreateProductRequest): Observable<ProductDetail> {
    return this.http.post<ProductDetail>(this.apiUrl, product);
  }

  updateProduct(id: number, product: UpdateProductRequest): Observable<ProductDetail> {
    return this.http.put<ProductDetail>(`${this.apiUrl}/${id}`, product);
  }

  deleteProduct(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  getAllCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(this.categoriesUrl);
  }
}