import { Injectable, Type, ComponentRef, ViewContainerRef } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { take } from 'rxjs/operators';

// Define the interface for the model comtent
export interface ModelContent<T = any, R = any> {
    data?: T;
    close: (result?: R) => void;
}

@Injectable({
    providedIn: 'root'
})
export class ModelService {
    
}