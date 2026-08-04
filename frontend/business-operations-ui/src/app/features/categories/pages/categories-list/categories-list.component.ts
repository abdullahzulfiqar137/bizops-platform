import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CategoryApiService } from '../../../../core/services/category-api.service';
import { CategoryResponse } from '../../models/category-response.model';

@Component({
  selector: 'app-categories-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './categories-list.component.html',
  styleUrl: './categories-list.component.css'
})
export class CategoriesListComponent implements OnInit {
  private categoryApi = inject(CategoryApiService);
  private fb = inject(FormBuilder);

  categories: CategoryResponse[] = [];
  showModal = false;
  editingCategory: CategoryResponse | null = null;

  categoryForm = this.fb.group({
    name: ['', Validators.required],
    categoryType: ['Product', Validators.required],
    parentCategory: ['None'],
    description: ['']
  });

  categoryTypes = [
    { label: '📦 Product (Physical Inventory)', value: 'Product' },
    { label: '⚙️ Service (Labor & Consulting)', value: 'Service' },
    { label: '🏭 Raw Material (Manufacturing)', value: 'Raw Material' },
    { label: '💸 Expense Category', value: 'Expense' },
    { label: '📂 General Category', value: 'General' }
  ];

  presetGroupMap: Record<string, Array<{ name: string; icon: string }>> = {
    'Electronics': [
      { name: 'Smartphones & Mobiles', icon: '📱' },
      { name: 'Laptops & Computers', icon: '💻' },
      { name: 'Audio & Headphones', icon: '🎧' },
      { name: 'Smart TVs & Displays', icon: '📺' },
      { name: 'Accessories & Chargers', icon: '🔌' },
      { name: 'Cameras & Drones', icon: '📷' }
    ],
    'Grocery & Food': [
      { name: 'Fresh Fruits & Vegetables', icon: '🍎' },
      { name: 'Dairy, Milk & Eggs', icon: '🥛' },
      { name: 'Rice, Grains & Pulses', icon: '🌾' },
      { name: 'Bakery, Biscuits & Snacks', icon: '🍞' },
      { name: 'Beverages & Soft Drinks', icon: '🥤' },
      { name: 'Meat, Seafood & Poultry', icon: '🍖' }
    ],
    'Clothing & Fashion': [
      { name: 'Men\'s Shirts & Trousers', icon: '👔' },
      { name: 'Women\'s Dresses & Suits', icon: '👗' },
      { name: 'Footwear & Shoes', icon: '👟' },
      { name: 'Jackets & Outerwear', icon: '🧥' },
      { name: 'Watches & Accessories', icon: '⌚' }
    ],
    'Healthcare & Pharmacy': [
      { name: 'Medicines & Tablets', icon: '💊' },
      { name: 'Medical Equipment & Devices', icon: '🩺' },
      { name: 'Personal Care & Hygiene', icon: '🧼' },
      { name: 'Vitamins & Supplements', icon: '🧪' }
    ],
    'Hardware & Tools': [
      { name: 'Power Tools & Hand Tools', icon: '🔨' },
      { name: 'Electrical & Wiring', icon: '⚡' },
      { name: 'Paints & Coatings', icon: '🎨' },
      { name: 'Plumbing & Fittings', icon: '🚰' }
    ],
    'Default': [
      { name: 'Electronics', icon: '📱' },
      { name: 'Grocery & Food', icon: '🍎' },
      { name: 'Clothing & Fashion', icon: '👗' },
      { name: 'Healthcare & Pharmacy', icon: '💊' },
      { name: 'Hardware & Tools', icon: '🔧' },
      { name: 'Office & Stationery', icon: '📚' },
      { name: 'Furniture & Decor', icon: '🛋️' }
    ]
  };

  get activePresetCategories(): Array<{ name: string; icon: string }> {
    const parentVal = (this.categoryForm.get('parentCategory')?.value || '').toLowerCase();
    if (!parentVal || parentVal === 'none') return []; // Hide suggestions until Parent Category is selected
    
    if (parentVal.includes('electr')) return this.presetGroupMap['Electronics'];
    if (parentVal.includes('groc') || parentVal.includes('food') || parentVal.includes('fruit')) return this.presetGroupMap['Grocery & Food'];
    if (parentVal.includes('cloth') || parentVal.includes('fashion') || parentVal.includes('wear')) return this.presetGroupMap['Clothing & Fashion'];
    if (parentVal.includes('health') || parentVal.includes('pharm') || parentVal.includes('medic')) return this.presetGroupMap['Healthcare & Pharmacy'];
    if (parentVal.includes('hardw') || parentVal.includes('tool')) return this.presetGroupMap['Hardware & Tools'];
    
    // Exact group match
    for (const key of Object.keys(this.presetGroupMap)) {
      if (key.toLowerCase() === parentVal) return this.presetGroupMap[key];
    }

    return [];
  }

  selectPresetName(name: string): void {
    this.categoryForm.patchValue({ name });
  }

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.categoryApi.getCategories().subscribe(res => {
      if (res.success && res.data) {
        this.categories = res.data;
      }
    });
  }

  openCreateModal(): void {
    this.editingCategory = null;
    this.categoryForm.reset({
      categoryType: 'Product',
      parentCategory: 'None'
    });
    this.showModal = true;
  }

  openEditModal(category: CategoryResponse): void {
    this.editingCategory = category;
    this.categoryForm.patchValue({
      name: category.name,
      categoryType: category.categoryType || 'Product',
      parentCategory: category.parentCategory || 'None',
      description: category.description || ''
    });
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.editingCategory = null;
  }

  saveCategory(): void {
    if (this.categoryForm.invalid) return;

    const val = this.categoryForm.value;
    if (this.editingCategory) {
      this.categoryApi.updateCategory(this.editingCategory.id, {
        name: val.name!,
        categoryType: val.categoryType,
        parentCategory: val.parentCategory === 'None' ? null : val.parentCategory,
        description: val.description || null,
        isActive: true
      }).subscribe(res => {
        if (res.success) {
          this.closeModal();
          this.loadCategories();
        }
      });
    } else {
      this.categoryApi.createCategory({
        name: val.name!,
        categoryType: val.categoryType,
        parentCategory: val.parentCategory === 'None' ? null : val.parentCategory,
        description: val.description || null
      }).subscribe(res => {
        if (res.success) {
          this.closeModal();
          this.loadCategories();
        }
      });
    }
  }

  deleteCategory(id: string): void {
    if (confirm('Are you sure you want to delete this category?')) {
      this.categoryApi.deleteCategory(id).subscribe(res => {
        if (res.success) {
          this.loadCategories();
        }
      });
    }
  }
}
