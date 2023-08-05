import { BrowserRouter as Router} from 'react-router-dom';
import Layout from './components/Layout/Layout';
import { LoadingProvider } from './contexts/LoadingContext/LoadingContext';
import Routes from './shared/Routes';
import { Toaster } from 'react-hot-toast';

function App() {
  return (
    <>
      <LoadingProvider>
        <Router>
          <Layout>
          <Toaster />
          <Routes></Routes>
          </Layout>
        </Router>
      </LoadingProvider>
    </>
  );
}

export default App;
