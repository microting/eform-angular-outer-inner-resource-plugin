import { innerResourcesPersistProvider } from './components/inner-resources/store';
import { outerResourcesPersistProvider } from './components/outer-resources/store';

export const outerInnerResourcesStoreProviders = [
  innerResourcesPersistProvider,
  outerResourcesPersistProvider,
];
