import PageContainer from "../../shared/PageContainer/PageContainer";
import './index.css';
import HomeForm from "./form/HomeForm";
import { useLoading } from '../../contexts/LoadingContext/LoadingContext';
import Loading from "../../components/Loading/Loading";

const HomePage = () => {
  const { isLoading } = useLoading();
  
    return (
      <PageContainer>
        {!isLoading ? (
          <div className="main-container">
            <div className="title-container">
              <h1>Welcome to SearchData App!</h1>
              <p className="site-desc">In the form below, you can perform a query to obtain the registration number for a respective CPF (Brazilian taxpayer identification number).</p>
            </div>
            <div className="form-container card">
              <h2 className="form-title">Search by CPF</h2>
              <div className="form-group-container">
                  <HomeForm />
              </div>
            </div>
          </div>
        ) : (
          <Loading></Loading>
        )}
      </PageContainer>
    );
  };
  
export default HomePage;