export interface ApiResponse<TData> {
  rtnCode: string;
  rtnMsg: string;
  data: TData;
}

export interface PagedResult<TItem> {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  items: TItem[];
}

export interface ErrorEnvelope {
  traceId?: string;
  path?: string;
  occurredAt?: string;
}