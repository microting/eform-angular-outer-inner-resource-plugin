export class OuterResourcesPnModel {
  total: number;
  areaList: Array<OuterResourcePnModel> = [];
  name: string;
}

export class OuterResourcePnModel {
  id: number;
  name: string;
  relatedMachinesIds: Array<number> = [];
}
