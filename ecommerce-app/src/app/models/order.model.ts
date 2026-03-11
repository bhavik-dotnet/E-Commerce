export interface OrderItem {
  orderItemId: number;
  productId: number;
  productName: string;
  photoUrl: string;
  quantity: number;
  price: number;
  subtotal: number;
}

export interface Order {
  orderId: number;
  totalAmount: number;
  orderDate: Date;
  orderStatus: string;
  items: OrderItem[];
}

export interface CreateOrderRequest {
  totalAmount: number;
}