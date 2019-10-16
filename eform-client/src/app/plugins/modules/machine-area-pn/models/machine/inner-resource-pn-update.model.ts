export class InnerResourcePnUpdateModel {
  id: number;
  name: string;
  relatedOuterResourcesIds: Array<number> = [];

  constructor(data?: any) {
    if (data) {
      this.id = data.id;
      this.name = data.name;
      this.relatedOuterResourcesIds = data.relatedOuterResourcesIds;
    }
  }
}
