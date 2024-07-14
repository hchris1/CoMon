import {
  Component,
  ElementRef,
  EventEmitter,
  Injector,
  Input,
  Output,
  ViewChild,
} from '@angular/core';
import {appModuleAnimation} from '@shared/animations/routerTransition';
import {AppComponentBase} from '@shared/app-component-base';
import {AssistantServiceProxy} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-assistant-modal',
  templateUrl: './assistant-modal.component.html',
  styleUrls: ['./assistant-modal.component.scss'],
  animations: [appModuleAnimation()],
})
export class AssistantModalComponent extends AppComponentBase {
  @ViewChild('messageContainer') private messageContainer: ElementRef;
  @Input() isRoot: boolean = false;
  @Input() groupId: number;
  @Input() assetId: number;
  @Input() statusId: number;
  @Output() onClose = new EventEmitter();

  chatId: string | undefined;
  messages: Message[] = [];
  userInput: string = '';
  examplePrompts: string[] = [
    'Assistant.ExamplePrompt1',
    'Assistant.ExamplePrompt2',
    'Assistant.ExamplePrompt3',
    'Assistant.ExamplePrompt4',
  ];
  isLoading: boolean = false;

  constructor(
    injector: Injector,
    private _assistantService: AssistantServiceProxy
  ) {
    super(injector);

    this.examplePrompts = this.examplePrompts.map(key =>
      this.localization.localize(key, this.localizationSourceName)
    );
  }

  sendMessage(prompt?: string): void {
    const message = prompt ? prompt : this.userInput.trim();
    if (!message) return;

    this.messages.push({text: message, sender: 'user'});
    this.scrollToBottom();

    this.userInput = '';
    this.getAnswer(message);
  }

  getAnswer(userMessage: string) {
    this.isLoading = true;
    this._assistantService
      .getAnswer(
        this.chatId,
        userMessage,
        this.assetId,
        this.groupId,
        this.statusId,
        this.isRoot
      )
      .subscribe(response => {
        this.userInput = '';
        this.chatId = response.chatId;
        this.messages.push({text: response.message, sender: 'bot'});
        this.scrollToBottom();
        this.isLoading = false;
      });
  }

  onCloseClicked() {
    this.onClose.emit();
  }

  scrollToBottom(): void {
    try {
      setTimeout(() => {
        this.messageContainer.nativeElement.scrollTop =
          this.messageContainer.nativeElement.scrollHeight;
      }, 0);
    } catch (err) {
      console.error(err);
    }
  }
}

interface Message {
  text: string;
  sender: 'user' | 'bot';
}
