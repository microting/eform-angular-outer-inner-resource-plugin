import {Component, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {} from 'src/app/plugins/modules/machine-area-pn/models/area';
import {OuterResourcePnCreateModel, InnerResourcesPnModel} from '../../../models';
import {OuterInnerResourcePnOuterResourceService} from '../../../services';

@Component({
  selector: 'app-machine-area-pn-area-create',
  templateUrl: './outer-resource-create.component.html',
  styleUrls: ['./outer-resource-create.component.scss']
})
export class OuterResourceCreateComponent implements OnInit {
  @ViewChild('frame') frame;
  @Input() mappingMachines: InnerResourcesPnModel = new InnerResourcesPnModel();
  @Output() onAreaCreated: EventEmitter<void> = new EventEmitter<void>();
  spinnerStatus = false;
  newAreaModel: OuterResourcePnCreateModel = new OuterResourcePnCreateModel();

  constructor(private machineAreaPnAreasService: OuterInnerResourcePnOuterResourceService) { }

  ngOnInit() {
  }
// tslint:disable-next-line:comment-format
//Hej med dig
  show() {
    this.frame.show();
  }

  createArea() {
    this.spinnerStatus = true;
    this.machineAreaPnAreasService.createArea(this.newAreaModel).subscribe((data) => {
      if (data && data.success) {
        this.onAreaCreated.emit();
        this.newAreaModel = new OuterResourcePnCreateModel();
        this.frame.hide();
      } this.spinnerStatus = false;
    });
  }

  addToArray(e: any, machineId: number) {
    if (e.target.checked) {
      this.newAreaModel.relatedMachinesIds.push(machineId);
    } else {
      this.newAreaModel.relatedMachinesIds = this.newAreaModel.relatedMachinesIds.filter(x => x !== machineId);
    }
  }

  isChecked(relatedMachineId: number) {
    return this.newAreaModel.relatedMachinesIds.indexOf(relatedMachineId) !== -1;
  }
}

