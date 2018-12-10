export class AreasPnModel {
  total: number;
  areaList: Array<AreaPnModel> = [];
}

export class AreaPnModel {
  id: number;
  name: string;
  relatedMachinesIds: Array<number> = [];
}
