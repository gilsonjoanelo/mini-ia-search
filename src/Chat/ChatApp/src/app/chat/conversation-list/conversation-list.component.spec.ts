import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConversationListComponent } from './conversation-list.component';

describe('ConversationList', () => {
  let component: ConversationListComponent;
  let fixture: ComponentFixture<ConversationListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ConversationListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ConversationListComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
