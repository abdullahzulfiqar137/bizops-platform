import { PaginationMetadata } from './pagination-metadata.model';

export interface PagedResponse<T> {
  success: boolean;
  message: string;
  data: T[];
  metadata: PaginationMetadata;
}
