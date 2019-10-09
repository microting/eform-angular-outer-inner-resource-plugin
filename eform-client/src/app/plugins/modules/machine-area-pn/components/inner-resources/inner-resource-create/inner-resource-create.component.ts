import {Component, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {
  MachineAreaPnAreasService,
  MachineAreaPnMachinesService
} from 'src/app/plugins/modules/machine-area-pn/services';
import {OuterResourcePnCreateModel, OuterResourcePnModel, OuterResourcesPnModel, InnerResourcePnCreateModel, InnerResourcesPnModel} from '../../../models';

@Component({
  selector: 'app-machine-area-pn-machine-create',
  templateUrl: './inner-resource-create.component.html',
  styleUrls: ['./inner-resource-create.component.scss']
})
export class InnerResourceCreateComponent implements OnInit {
  @ViewChild('frame') frame;
  @Input() mappingAreas: OuterResourcesPnModel = new OuterResourcesPnModel();
  @Output() onMachineCreated: EventEmitter<void> = new EventEmitter<void>();
  checked = false;
  spinnerStatus = false;
  newMachineModel: InnerResourcePnCreateModel = new InnerResourcePnCreateModel();

  constructor(private machineAreaPnMachinesService: MachineAreaPnMachinesService) { }

  ngOnInit() {

  }

  show() {
    this.frame.show();
  }

  createMachine() {
    this.spinnerStatus = true;
    this.machineAreaPnMachinesService.createMachine(this.newMachineModel).subscribe((data) => {
      if (data && data.success) {
        this.onMachineCreated.emit();
        this.newMachineModel = new InnerResourcePnCreateModel();
        this.frame.hide();
      }
      this.spinnerStatus = false;
    });
  }

  addToArray(e: any, areaId: number) {
    if (e.target.checked) {
      this.newMachineModel.relatedAreasIds.push(areaId);
    } else {
      this.newMachineModel.relatedAreasIds = this.newMachineModel.relatedAreasIds.filter(x => x !== areaId);
    }
  }

  isChecked(relatedAreaId: number) {
    return this.newMachineModel.relatedAreasIds.indexOf(relatedAreaId) !== -1;
  }
}
