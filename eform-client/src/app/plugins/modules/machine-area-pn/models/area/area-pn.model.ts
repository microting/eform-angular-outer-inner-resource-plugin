export class AreasPnModel {
  total: number;
  areas: Array<AreaPnModel> = [];
}

export class AreaPnModel {
  id: number;
  name: string;
  createdAt: Date;
  updatedAt: Date;
  workflowState: string;
  createdByUserId: number;
  updatedByUserId: number;
}
