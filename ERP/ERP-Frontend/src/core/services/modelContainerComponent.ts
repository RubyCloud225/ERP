import { Component, ViewChild, ViewContainerRef, OnInit, OnDestroy } from '@angular/core';
import { ModelService } from '../services/model.service';
import { Subscription } from 'rxjs';
import { NgIf } from '@angular/common';

@Component({
    selector: 'app-model-container',
    template: `
    <div class="model-backdrop" *ngIf="isModelOpen" (click)="onBackdropClick()"></div>
    <ng-template #modelOutlet></ng-template>
  `,
    styleUrls: ['./modelContainerComponent.scss'],
    providers: [ModelService],
    imports: [NgIf],
    standalone: true // Set to false if this component is part of a module
})
export class ModelContainerComponent implements OnInit, OnDestroy {
    @ViewChild('modelOutlet', { read: ViewContainerRef, static: true }) modelOutlet!: ViewContainerRef;
    isModelOpen: boolean = false;
    private subscription?: Subscription;

    constructor(private modelService: ModelService) {}

    ngOnInit(): void {
        this.modelService.setModalContainer(this.modelOutlet);
        this.subscription = this.modelService.isAnyModelOpen$.subscribe((isOpen: boolean) => {
            this.isModelOpen = isOpen;
            if (isOpen) {
                document.body.style.overflow = 'hidden';
            } else {
                document.body.style.overflow = '';
            }
        });
    }


    onBackdropClick(): void {
        this.modelService.closeAllModels();
    }
    ngOnDestroy(){
        this.subscription?.unsubscribe();
    }
}
