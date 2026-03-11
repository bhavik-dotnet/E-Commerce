import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface FileUploadResponse {
  success: boolean;
  fileUrl: string;
  fileName: string;
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class FileUploadService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/fileupload`;

  uploadFile(file: File): Observable<FileUploadResponse> {
    const formData = new FormData();
    formData.append('file', file);

    // Include JWT token if available
    const token = localStorage.getItem('token'); // or wherever you store it
    const headers = token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : undefined;

    return this.http.post<FileUploadResponse>(`${this.apiUrl}/upload`, formData, { headers });
  }

  deleteFile(fileName: string): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : undefined;

    return this.http.delete(`${this.apiUrl}/delete?fileName=${fileName}`, { headers });
  }
}
