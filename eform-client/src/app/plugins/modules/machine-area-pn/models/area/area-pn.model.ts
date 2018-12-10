export class AreasPnModel {
  total: number;
  areas: Array<AreaPnModel> = [];
}

export class AreaPnModel {
  id: number;
  name: string;
  relatedMachinesIds: Array<number> = [];
}
