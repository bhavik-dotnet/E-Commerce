import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { AuthService } from '../../services/auth.service';
import { Product, Category, CreateProductRequest } from '../../models/product.model';

import { FileUploadService } from '../../services/file-upload.service';
import { environment } from '../../../environments/environment';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  private fb = inject(FormBuilder);
  private productService = inject(ProductService);
  private authService = inject(AuthService);
  private router = inject(Router);

  private fileUploadService = inject(FileUploadService);

  productForm!: FormGroup;
  products: Product[] = [];
  categories: Category[] = [];
  isEditMode = false;
  currentProductId: number | null = null;
  loading = false;
  errorMessage = '';
  successMessage = '';

  // File upload related
  uploadMode: 'file' | 'url' = 'file';
  selectedFile: File | null = null;
  imagePreview: string | null = null;
  uploadingFile = false;

  ngOnInit(): void {
    this.initForm();
    this.loadProducts();
    this.loadCategories();
  }

  initForm(): void {
    this.productForm = this.fb.group({
      productName: ['', [Validators.required, Validators.maxLength(200)]],
     photoUrl: [''],
      quantity: [1, [Validators.required, Validators.min(1), Validators.max(10)]],
      price: [0, [Validators.required, Validators.min(0.01)]],
      categoryIds: [[]]
    });
  }

  get f() {
    return this.productForm.controls;
  }

  loadProducts(): void {
    this.productService.getAllProducts().subscribe({
      next: (products) => this.products = products,
      error: (error) => this.errorMessage = 'Failed to load products'
    });
  }

  loadCategories(): void {
    this.productService.getAllCategories().subscribe({
      next: (categories) => this.categories = categories,
      error: (error) => this.errorMessage = 'Failed to load categories'
    });
  }

  onCategoryChange(categoryId: number, event: any): void {
    const categoryIds = this.productForm.get('categoryIds')?.value || [];
    if (event.target.checked) {
      categoryIds.push(categoryId);
    } else {
      const index = categoryIds.indexOf(categoryId);
      if (index > -1) {
        categoryIds.splice(index, 1);
      }
    }
    this.productForm.patchValue({ categoryIds });
  }

  isCategorySelected(categoryId: number): boolean {
    const categoryIds = this.productForm.get('categoryIds')?.value || [];
    return categoryIds.includes(categoryId);
  }

  async onSubmit(): Promise<void> {
  console.log(" onSubmit called");

  if (this.productForm.invalid) {
    console.log(" Form invalid", this.productForm.value);
    Object.keys(this.productForm.controls).forEach(key => {
      this.productForm.controls[key].markAsTouched();
    });
    return;
  }

  const categoryIds = this.productForm.get('categoryIds')?.value;
  console.log(" Selected categories:", categoryIds);
  if (!categoryIds || categoryIds.length === 0) {
    this.errorMessage = 'Please select at least one category';
    return;
  }

  this.loading = true;
  console.log(" Loading started...");

  let photoUrl = this.productForm.get('photoUrl')?.value;
  console.log(" Initial photoUrl:", photoUrl);

  if (this.uploadMode === 'file') {
    console.log(" Upload mode = file, selectedFile:", this.selectedFile);
    if (!this.selectedFile) {
      console.log(" No file selected, stopping");
      this.errorMessage = 'Please select an image file';
      this.loading = false;
      return;
    }

    const uploadedUrl = await this.uploadFile();
    console.log(" Uploaded URL:", uploadedUrl);
    if (!uploadedUrl) {
      console.log(" Upload failed, stopping");
      this.loading = false;
      return;
    }

    this.productForm.patchValue({ photoUrl: uploadedUrl });
    photoUrl = uploadedUrl;
  }

  console.log(" Final product data:", { ...this.productForm.value, photoUrl });

  const request$ = this.isEditMode && this.currentProductId
    ? this.productService.updateProduct(this.currentProductId, { ...this.productForm.value, photoUrl })
    : this.productService.createProduct({ ...this.productForm.value, photoUrl });

  request$.subscribe({
    next: () => {
      console.log(" API success");
      this.successMessage = this.isEditMode ? 'Product updated successfully' : 'Product created successfully';
      this.resetForm();
      this.loadProducts();
    },
    error: (error) => {
      console.error(" API error:", error);
      this.errorMessage = error.error?.message || (this.isEditMode ? 'Failed to update product' : 'Failed to create product');
      this.loading = false;
    }
  });
}



  editProduct(product: Product): void {
    this.isEditMode = true;
    this.currentProductId = product.productId;
    
    this.productService.getProductById(product.productId).subscribe({
      next: (productDetail) => {
        this.productForm.patchValue({
          productName: productDetail.productName,
          photoUrl: productDetail.photoUrl,
          quantity: productDetail.quantity,
          price: productDetail.price,
          categoryIds: productDetail.categories.map(c => c.categoryId)
        });
      },
      error: () => this.errorMessage = 'Failed to load product details'
    });
  }

  deleteProduct(productId: number): void {
    if (confirm('Are you sure you want to delete this product?')) {
      this.productService.deleteProduct(productId).subscribe({
        next: () => {
          this.successMessage = 'Product deleted successfully';
          this.loadProducts();
        },
        error: () => this.errorMessage = 'Failed to delete product'
      });
    }
  }

  resetForm(): void {
    this.isEditMode = false;
    this.currentProductId = null;
    this.productForm.reset({ quantity: 1, price: 0, categoryIds: [] });
    this.selectedFile = null;
    this.imagePreview = null;
    this.uploadMode = 'file';
    this.loading = false;
    this.uploadingFile = false;
    const fileInput = document.getElementById('fileInput') as HTMLInputElement;
    if (fileInput) {
      fileInput.value = '';
    }
  }

  logout(): void {
    this.authService.logout();
  }

  goToCustomerView(): void {
    this.router.navigate(['/customer/products']);
  }

  // --- File upload logic ---
 switchUploadMode(mode: 'file' | 'url') {
  this.uploadMode = mode;
  const photoUrlControl = this.productForm.get('photoUrl');

  if (mode === 'url') {
    photoUrlControl?.setValidators([Validators.required]);
  } else {
    photoUrlControl?.clearValidators();
    photoUrlControl?.setValue('');
  }
  photoUrlControl?.updateValueAndValidity();
}


  onFileSelected(event: any) {
    const file = event.target.files && event.target.files[0];
    if (!file) return;
    if (!file.type.startsWith('image/')) {
      this.errorMessage = 'Only image files are allowed';
      return;
    }
    if (file.size > 5 * 1024 * 1024) {
      this.errorMessage = 'File size must be less than 5MB';
      return;
    }
    this.selectedFile = file;
    const reader = new FileReader();
    reader.onload = (e: any) => {
      this.imagePreview = e.target.result;
    };
    reader.readAsDataURL(file);
    this.errorMessage = '';
  }

  removeSelectedFile() {
    this.selectedFile = null;
    this.imagePreview = null;
    const fileInput = document.getElementById('fileInput') as HTMLInputElement;
    if (fileInput) fileInput.value = '';
  }

  async uploadFile(): Promise<string | null> {
    if (!this.selectedFile) return null;
    this.uploadingFile = true;
    try {
      const response: any = await firstValueFrom(
        this.fileUploadService.uploadFile(this.selectedFile)
      );
      this.uploadingFile = false;
      if (!response || !response.fileUrl) {
        this.errorMessage = 'File upload failed: no URL returned';
        return null;
      }
      return response.fileUrl;
    } catch (error: any) {
      this.errorMessage = error.error?.message || 'File upload failed';
      this.uploadingFile = false;
      return null;
    }
  }
}