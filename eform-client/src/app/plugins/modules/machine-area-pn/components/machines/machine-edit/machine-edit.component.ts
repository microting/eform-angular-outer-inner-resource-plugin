import {Component, EventEmitter, OnInit, Output, ViewChild} from '@angular/core';

@Component({
  selector: 'app-machine-area-pn-machine-edit',
  templateUrl: './machine-edit.component.html',
  styleUrls: ['./machine-edit.component.scss']
})
export class MachineEditComponent implements OnInit {
  @ViewChild('frame') frame;
  @Output() onNewMachineUpdated: EventEmitter<void> = new EventEmitter<void>();
  spinnerStatus = false;
  selectedMachineModel: {
    id: 1,
    name: ''
  };
  constructor() { }

  ngOnInit() {
  }

  show() {
    this.frame.show();
  }

  updateMachine() {

  }

}
