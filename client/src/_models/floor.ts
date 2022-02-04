import { WorkPlace } from "./workplace";

export interface Floor{
    id: number;
    workPlaces: WorkPlace[];
    floorNumber: number;
}