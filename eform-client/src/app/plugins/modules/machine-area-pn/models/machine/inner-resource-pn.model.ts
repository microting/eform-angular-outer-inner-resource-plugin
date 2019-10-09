export class InnerResourcesPnModel {
  total: number;
  machineList: Array<InnerResourcePnModel> = [];
  name: string;
}

export class InnerResourcePnModel {
  id: number;
  name: string;
  relatedAreasIds: Array<number> = [];
}
