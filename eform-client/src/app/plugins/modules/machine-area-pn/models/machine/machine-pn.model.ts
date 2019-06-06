export class MachinesPnModel {
  total: number;
  machineList: Array<MachinePnModel> = [];
  name: string;
}

export class MachinePnModel {
  id: number;
  name: string;
  relatedAreasIds: Array<number> = [];
}
