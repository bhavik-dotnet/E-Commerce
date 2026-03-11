export interface Rating {
  ratingId: number;
  productId: number;
  userId: number;
  username: string;
  rating: number;
  review?: string;
  createdDate: Date;
}

export interface CreateRatingRequest {
  productId: number;
  rating: number;
  review?: string;
}