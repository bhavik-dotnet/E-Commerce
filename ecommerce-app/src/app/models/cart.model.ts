export interface CartItem {
  cartItemId: number;
  productId: number;
  productName: string;
  photoUrl: string;
  quantity: number;
  price: number;
  subtotal: number;
  availableQuantity: number;
}

export interface Cart {
  items: CartItem[];
  totalAmount: number;
  totalItems: number;
}

export interface AddToCartRequest {
  productId: number;
  quantity: number;
}

export interface UpdateCartItemRequest {
  quantity: number;
}