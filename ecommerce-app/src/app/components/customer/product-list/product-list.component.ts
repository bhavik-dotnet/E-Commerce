import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../../services/product.service';
import { CartService } from '../../../services/cart.service';
import { RatingService } from '../../../services/rating.service';
import { AuthService } from '../../../services/auth.service';
import { Product, Category, ProductFilter } from '../../../models/product.model';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {
  private productService = inject(ProductService);
  private cartService = inject(CartService);
  private ratingService = inject(RatingService);
  private authService = inject(AuthService);
  private router = inject(Router);

  products: Product[] = [];
  categories: Category[] = [];
  cartItemCount = 0;
  selectedProduct: Product | null = null;
  selectedRating = 0;
  ratingReview = '';
  addingToCart: { [key: number]: boolean } = {};
  successMessage = '';
  errorMessage = '';
  loading = false;

  // Filter properties
  searchTerm = '';
  selectedCategories: number[] = [];
  minPrice: number | null = null;
  maxPrice: number | null = null;
  inStockOnly = false;
  sortBy = 'sold';
  sortOrder = 'desc';
  
  // Pagination
  currentPage = 1;
  pageSize = 12;
  totalCount = 0;
  totalPages = 0;
  hasPreviousPage = false;
  hasNextPage = false;

  showFilters = false;

  Math = Math;

  ngOnInit(): void {
    this.loadCategories();
    this.loadProducts();
    this.loadCart();
  }

  loadCategories(): void {
    this.productService.getAllCategories().subscribe({
      next: (categories) => this.categories = categories,
      error: () => this.errorMessage = 'Failed to load categories'
    });
  }

  loadProducts(): void {
    this.loading = true;
    const filter: ProductFilter = {
      searchTerm: this.searchTerm || undefined,
      categoryIds: this.selectedCategories.length > 0 ? this.selectedCategories : undefined,
      minPrice: this.minPrice ?? undefined,
      maxPrice: this.maxPrice ?? undefined,
      inStock: this.inStockOnly ? true : undefined,
      sortBy: this.sortBy,
      sortOrder: this.sortOrder,
      pageNumber: this.currentPage,
      pageSize: this.pageSize
    };

    this.productService.getFilteredProducts(filter).subscribe({
      next: (response) => {
        this.products = response.products;
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
        this.hasPreviousPage = response.hasPreviousPage;
        this.hasNextPage = response.hasNextPage;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load products';
        this.loading = false;
      }
    });
  }

  onSearch(): void {
    this.currentPage = 1;
    this.loadProducts();
  }

  onCategoryChange(categoryId: number, event: any): void {
    if (event.target.checked) {
      this.selectedCategories.push(categoryId);
    } else {
      const index = this.selectedCategories.indexOf(categoryId);
      if (index > -1) {
        this.selectedCategories.splice(index, 1);
      }
    }
    this.currentPage = 1;
    this.loadProducts();
  }

  isCategorySelected(categoryId: number): boolean {
    return this.selectedCategories.includes(categoryId);
  }

  onPriceFilter(): void {
    this.currentPage = 1;
    this.loadProducts();
  }

  onStockFilterChange(): void {
    this.currentPage = 1;
    this.loadProducts();
  }

  onSortChange(): void {
    this.currentPage = 1;
    this.loadProducts();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.selectedCategories = [];
    this.minPrice = null;
    this.maxPrice = null;
    this.inStockOnly = false;
    this.sortBy = 'sold';
    this.sortOrder = 'desc';
    this.currentPage = 1;
    this.loadProducts();
  }

  toggleFilters(): void {
    this.showFilters = !this.showFilters;
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadProducts();
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  getPageNumbers(): number[] {
    const pages: number[] = [];
    const maxPagesToShow = 5;
    let startPage = Math.max(1, this.currentPage - Math.floor(maxPagesToShow / 2));
    let endPage = Math.min(this.totalPages, startPage + maxPagesToShow - 1);

    if (endPage - startPage < maxPagesToShow - 1) {
      startPage = Math.max(1, endPage - maxPagesToShow + 1);
    }

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    return pages;
  }

  loadCart(): void {
    this.cartService.getCart().subscribe({
      next: (cart) => this.cartItemCount = cart.totalItems
    });
  }

  addToCart(product: Product): void {
    if (product.quantity === 0) {
      return;
    }

    this.addingToCart[product.productId] = true;
    this.cartService.addToCart({ productId: product.productId, quantity: 1 }).subscribe({
      next: () => {
        this.successMessage = `${product.productName} added to cart`;
        this.loadCart();
        this.addingToCart[product.productId] = false;
        setTimeout(() => this.successMessage = '', 3000);
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to add to cart';
        this.addingToCart[product.productId] = false;
        setTimeout(() => this.errorMessage = '', 3000);
      }
    });
  }

  openRatingModal(product: Product): void {
    this.selectedProduct = product;
    this.selectedRating = 0;
    this.ratingReview = '';
  }

  setRating(rating: number): void {
    this.selectedRating = rating;
  }

  submitRating(): void {
    if (!this.selectedProduct || this.selectedRating === 0) {
      return;
    }

    this.ratingService.createOrUpdateRating({
      productId: this.selectedProduct.productId,
      rating: this.selectedRating,
      review: this.ratingReview || undefined
    }).subscribe({
      next: () => {
        this.successMessage = 'Rating submitted successfully';
        this.closeRatingModal();
        this.loadProducts();
        setTimeout(() => this.successMessage = '', 3000);
      },
      error: () => {
        this.errorMessage = 'Failed to submit rating';
        setTimeout(() => this.errorMessage = '', 3000);
      }
    });
  }

  closeRatingModal(): void {
    this.selectedProduct = null;
    this.selectedRating = 0;
    this.ratingReview = '';
  }

  getStarArray(rating: number): number[] {
    return Array(5).fill(0).map((_, i) => i < Math.round(rating) ? 1 : 0);
  }

  goToCart(): void {
    this.router.navigate(['/customer/cart']);
  }

  goToOrders(): void {
    this.router.navigate(['/customer/orders']);
  }

  logout(): void {
    this.authService.logout();
  }
}