import {ReportEntityHeaderModel} from './report-entity-header.model';
import {ReportEntityModel} from './report-entity.model';

export class ReportPnFullModel {
  reportHeaders: Array<ReportEntityHeaderModel> = [];
  entities: Array<ReportEntityModel> = [];
  totalTime: number;
  totalTimePerTimeUnit: number[];
}
