import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<MemberEditComponent> {
  canDeactivate(component: MemberEditComponent): boolean {
    if(component.editForm?.dirty){
      return confirm("Sei sicuro di voler continuare? Le modifiche non salvate verranno perse");
    }
    return true;
  }  
}
