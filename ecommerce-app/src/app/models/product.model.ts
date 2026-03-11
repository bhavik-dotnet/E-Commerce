export interface Category {
  categoryId: number;
  categoryName: string;
}

export interface Product {
  productId: number;
  productName: string;
  photoUrl: string;
  quantity: number;
  price: number;
  soldCount: number;
  isSoldOut: boolean;
  categories: Category[];
  averageRating: number;
  totalRatings: number;
}

export interface ProductDetail extends Product {
  categories: Category[];
  createdDate: Date;
}

export interface CreateProductRequest {
  productName: string;
  photoUrl: string;
  quantity: number;
  price: number;
  categoryIds: number[];
}

export interface UpdateProductRequest {
  productName: string;
  photoUrl: string;
  quantity: number;
  price: number;
  categoryIds: number[];
}

export interface ProductFilter {
  searchTerm?: string;
  categoryIds?: number[];
  minPrice?: number;
  maxPrice?: number;
  inStock?: boolean;
  sortBy?: string;
  sortOrder?: string;
  pageNumber: number;
  pageSize: number;
}

export interface PagedProductResponse {
  products: Product[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}