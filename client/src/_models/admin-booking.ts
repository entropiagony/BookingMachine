export interface AdminBooking {
    id: number;
    workPlaceId: number;
    bookingDate: Date;
    status: string;
    employeeFirstName: string;
    employeeLastName: string;
    managerFirstName: string;
    managerLastName: string;
}