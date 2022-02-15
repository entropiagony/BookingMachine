export interface Report {
    id: number;
    workPlaceId: number;
    bookingDate: Date;
    status: string;
    employee: { firstName: string, lastName: string };
    manager: { firstName: string, lastName: string };
    floorNumber: number;
    createdDate: Date;
    managedDate: Date;
    reason: string;
}