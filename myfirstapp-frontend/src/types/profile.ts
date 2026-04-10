export interface ApplicationProfileDto {
  profileKey: string;
  displayName: string;
  ownerTeam: string;
  environment: string;
  isActive: boolean;
  updatedAt: string;
}

export interface ApplicationProfileQuery {
  keyword?: string;
  pageNumber?: number;
  pageSize?: number;
  isActive?: boolean;
}

export interface CreateApplicationProfileCommand {
  profileKey: string;
  displayName: string;
  ownerTeam: string;
  environment: string;
  isActive: boolean;
}

export interface UpdateApplicationProfileCommand {
  displayName: string;
  ownerTeam: string;
  environment: string;
  isActive: boolean;
}