export interface NewsButton {
  text: string;
  url: string;
}

export interface NewsEvent {
  telegramIds: number[];
  message: string;
  buttons: NewsButton[];
}
