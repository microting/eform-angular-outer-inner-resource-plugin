export class OuterResourcePnUpdateModel {
  id: number;
  name: string;
  relatedInnerResourcesIds: Array<number> = [];

  constructor(data?: any) {
    if (data) {
      this.id = data.id;
      this.name = data.name;
      this.relatedInnerResourcesIds = data.relatedInnerResourcesIds;
    }
  }
}
