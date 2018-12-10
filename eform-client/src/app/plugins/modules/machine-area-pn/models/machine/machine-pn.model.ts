export class MachinesPnModel {
  total: number;
  machineList: Array<MachinePnModel> = [];
}

export class MachinePnModel {
  id: number;
  name: string;
  relatedAreasIds: Array<number> = [];
}
