export interface Person {
  id: string;
  firstName: string;
  lastName: string;
  company?: string;
  createdAt: Date;
  updatedAt: Date;
  contactInfos: ContactInfo[];
}

export interface PersonSummary {
  id: string;
  firstName: string;
  lastName: string;
  company?: string;
  contactInfoCount: number;
}

export interface ContactInfo {
  id: string;
  type: ContactType;
  content: string;
  createdAt: Date;
}

export enum ContactType {
  PhoneNumber = 1,
  EmailAddress = 2,
  Location = 3
}

export interface CreatePersonRequest {
  firstName: string;
  lastName: string;
  company?: string;
}

export interface AddContactInfoRequest {
  type: ContactType;
  content: string;
}

export const ContactTypeLabels = {
  [ContactType.PhoneNumber]: 'Telefon',
  [ContactType.EmailAddress]: 'E-posta',
  [ContactType.Location]: 'Konum'
};

export const ContactTypeIcons = {
  [ContactType.PhoneNumber]: 'phone',
  [ContactType.EmailAddress]: 'email',
  [ContactType.Location]: 'location_on'
};
