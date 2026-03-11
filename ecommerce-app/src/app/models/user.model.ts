export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  userId: number;
  username: string;
  email: string;
  fullName: string;
  isAdmin: boolean;
  token: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  fullName: string;
}

export interface RegisterResponse {
  success: boolean;
  message: string;
  user?: LoginResponse;
}

export interface User {
  userId: number;
  username: string;
  email: string;
  fullName: string;
  isAdmin: boolean;
}