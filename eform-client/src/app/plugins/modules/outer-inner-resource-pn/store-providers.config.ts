import { innerResourcesPersistProvider } from './components/inner-resources/store/inner-resources-store';
import { outerResourcesPersistProvider } from './components/outer-resources/store/outer-resources-store';

export const outerInnerResourcesStoreProviders = [
  innerResourcesPersistProvider,
  outerResourcesPersistProvider,
];
