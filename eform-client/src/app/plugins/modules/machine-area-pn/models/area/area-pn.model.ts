export class AreasPnModel {
  total: number;
  areaList: Array<AreaPnModel> = [];
  name: string;
}

export class AreaPnModel {
  id: number;
  name: string;
  relatedMachinesIds: Array<number> = [];
}
