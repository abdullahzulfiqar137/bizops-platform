import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ProductApiService } from '../../../../core/services/product-api.service';
import { CategoryApiService } from '../../../../core/services/category-api.service';
import { ProductResponse } from '../../models/product-response.model';
import { CategoryResponse } from '../../../categories/models/category-response.model';
import { ProductUnitResponse } from '../../../product-units/models/product-unit-response.model';

@Component({
  selector: 'app-products-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './products-list.component.html',
  styleUrl: './products-list.component.css'
})
export class ProductsListComponent implements OnInit {
  private productApi = inject(ProductApiService);
  private categoryApi = inject(CategoryApiService);
  private fb = inject(FormBuilder);

  products: ProductResponse[] = [];
  categories: CategoryResponse[] = [];
  units: ProductUnitResponse[] = [];

  searchQuery = '';
  selectedCategory = '';
  isLowStockOnly = false;

  currentPage = 1;
  pageSize = 10;
  totalPages = 1;
  totalCount = 0;
  showFormModal = false;
  editingProduct: ProductResponse | null = null;
  successMessage = '';
  newlyAddedProductId: string | null = null;

  productForm = this.fb.group({
    name: ['', Validators.required],
    barcode: [''],
    categoryId: ['', Validators.required],
    unitId: ['', Validators.required],
    costPrice: [0, [Validators.required, Validators.min(0)]],
    salePrice: [0, [Validators.required, Validators.min(0)]],
    minimumStockLevel: [5, [Validators.required, Validators.min(0)]],
    initialStockQuantity: [0],
    description: ['']
  });

  productPresets = [
    { name: 'Wireless Bluetooth Earbuds', cost: 12.0, price: 25.0, icon: '🎧' },
    { name: 'USB-C Fast Charging Cable (2m)', cost: 3.5, price: 9.99, icon: '🔌' },
    { name: 'Men\'s Premium Cotton Polo T-Shirt', cost: 8.0, price: 19.99, icon: '👕' },
    { name: 'Organic Extra Virgin Olive Oil (1L)', cost: 6.5, price: 14.99, icon: '🍾' },
    { name: 'Ergonomic Executive Office Chair', cost: 45.0, price: 110.0, icon: '🪑' },
    { name: 'Smart Fitness Tracker Watch', cost: 18.0, price: 42.0, icon: '⌚' }
  ];

  selectProductPreset(p: { name: string; cost: number; price: number }): void {
    this.productForm.patchValue({
      name: p.name,
      costPrice: p.cost,
      salePrice: p.price
    });
  }

  generateRandomBarcode(): void {
    const code = 'PROD-' + Math.floor(100000000000 + Math.random() * 900000000000);
    this.productForm.patchValue({ barcode: code });
  }

  ngOnInit(): void {
    this.loadProducts();
    this.loadCategories();
    this.loadUnits();
  }

  loadProducts(): void {
    this.productApi.getProducts(
      this.searchQuery,
      this.selectedCategory || undefined,
      undefined,
      this.isLowStockOnly ? true : undefined,
      undefined,
      true,
      this.currentPage,
      this.pageSize
    ).subscribe(res => {
      if (res.success) {
        this.products = res.data;
        this.currentPage = res.metadata.currentPage;
        this.totalPages = res.metadata.totalPages;
        this.totalCount = res.metadata.totalCount;
      }
    });
  }

  loadCategories(): void {
    this.categoryApi.getCategories().subscribe(res => {
      if (res.success && res.data) this.categories = res.data;
    });
  }

  loadUnits(): void {
    this.productApi.getProductUnits().subscribe(res => {
      if (res.success && res.data) this.units = res.data;
    });
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadProducts();
    }
  }

  openCreateModal(): void {
    this.editingProduct = null;
    this.productForm.reset({ costPrice: 0, salePrice: 0, minimumStockLevel: 5, initialStockQuantity: 0 });
    this.showFormModal = true;
  }

  openEditModal(product: ProductResponse): void {
    this.editingProduct = product;
    this.productForm.patchValue({
      name: product.name,
      barcode: product.barcode || '',
      categoryId: product.categoryId,
      unitId: product.unitId,
      costPrice: product.costPrice,
      salePrice: product.salePrice,
      minimumStockLevel: product.minimumStockLevel,
      description: product.description || ''
    });
    this.showFormModal = true;
  }

  closeFormModal(): void {
    this.showFormModal = false;
    this.editingProduct = null;
  }

  saveProduct(): void {
    if (this.productForm.invalid) return;

    const val = this.productForm.value;
    if (this.editingProduct) {
      this.productApi.updateProduct(this.editingProduct.id, {
        name: val.name!,
        barcode: val.barcode || null,
        description: val.description || null,
        categoryId: val.categoryId!,
        unitId: val.unitId!,
        costPrice: val.costPrice!,
        salePrice: val.salePrice!,
        minimumStockLevel: val.minimumStockLevel!,
        isActive: true
      }).subscribe(res => {
        if (res.success && res.data) {
          this.closeFormModal();
          this.newlyAddedProductId = res.data.id;
          this.successMessage = `Product "${res.data.name}" (SKU: ${res.data.sku}) updated successfully!`;
          this.loadProducts();
        }
      });
    } else {
      this.productApi.createProduct({
        name: val.name!,
        barcode: val.barcode || null,
        description: val.description || null,
        categoryId: val.categoryId!,
        unitId: val.unitId!,
        costPrice: val.costPrice!,
        salePrice: val.salePrice!,
        minimumStockLevel: val.minimumStockLevel!,
        initialStockQuantity: val.initialStockQuantity || 0
      }).subscribe(res => {
        if (res.success && res.data) {
          this.closeFormModal();
          this.newlyAddedProductId = res.data.id;
          this.successMessage = `Product "${res.data.name}" (SKU: ${res.data.sku}) successfully created and listed below!`;
          this.currentPage = 1;
          this.loadProducts();
        }
      });
    }
  }

  deleteProduct(id: string): void {
    if (confirm('Are you sure you want to delete this product?')) {
      this.productApi.deleteProduct(id).subscribe(res => {
        if (res.success) {
          this.loadProducts();
        }
      });
    }
  }
}
