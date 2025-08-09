export interface Report {
  id: string;
  requestedAt: Date;
  status: ReportStatus;
  completedAt?: Date;
  errorMessage?: string;
  locationStatistics: LocationStatistic[];
}

export interface LocationStatistic {
  location: string;
  personCount: number;
  phoneNumberCount: number;
}

export enum ReportStatus {
  Preparing = 1,
  Completed = 2,
  Failed = 3
}

export const ReportStatusLabels = {
  [ReportStatus.Preparing]: 'Hazırlanıyor',
  [ReportStatus.Completed]: 'Tamamlandı',
  [ReportStatus.Failed]: 'Başarısız'
};

export const ReportStatusColors = {
  [ReportStatus.Preparing]: 'warn',
  [ReportStatus.Completed]: 'primary',
  [ReportStatus.Failed]: 'accent'
};

export const ReportStatusIcons = {
  [ReportStatus.Preparing]: 'hourglass_empty',
  [ReportStatus.Completed]: 'check_circle',
  [ReportStatus.Failed]: 'error'
};

