import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Rating, CreateRatingRequest } from '../models/rating.model';

@Injectable({
  providedIn: 'root'
})
export class RatingService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/ratings`;

  getProductRatings(productId: number): Observable<Rating[]> {
    return this.http.get<Rating[]>(`${this.apiUrl}/product/${productId}`);
  }

  createOrUpdateRating(request: CreateRatingRequest): Observable<Rating> {
    return this.http.post<Rating>(this.apiUrl, request);
  }
}