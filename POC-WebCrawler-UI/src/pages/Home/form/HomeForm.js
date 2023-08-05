import React, { useState } from 'react';
import * as Yup from 'yup';
import './HomeForm.css';
import InputMask from 'react-input-mask';
import { getData } from '../../../services/CustomerApiService';

const HomeForm = () => {

  const [cpf, setCpf] = useState('');
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [data, setData] = useState('');
  const [hasResult, setHasResult] = useState(false);

  const validationSchema = Yup.object({
    cpf: Yup.string()
      .matches(/^\d{3}\.\d{3}\.\d{3}-\d{2}$/, 'Invalid CPF')
      .required('Field CPF is required'),
  });

  const handleChange = (e) => {
    setError('');
    setCpf(e.target.value);
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    setIsLoading(true);
    
    validationSchema
      .validate({ cpf })
      .then(async () => {
        const responseData = await getData(`/register/${cpf}`);
        if(responseData !== null && responseData !== ''){
          setData(responseData);
          setHasResult(true);
        }
        setCpf('');
        setIsLoading(false);
      })
      .catch((err) => {
        if(err.response?.status === 404){
          setHasResult(true);
        }
        else{
          setError(err.message);
          setIsLoading(false);
        }
      });
  };

  const resetForm = () => {
    setCpf('');
    setIsLoading(false);
    setHasResult(false);
    setData('');
  }

  return (
    <div className="main-container">
      {!hasResult ?
        <form onSubmit={handleSubmit}>
          <div className="home-form-group">
            <div className="input-form-group">
              <InputMask
                className="form-control"
                type="text" 
                id="cpf" 
                name="cpf" 
                value={cpf} 
                onChange={handleChange} 
                placeholder="CPF"
                mask="999.999.999-99"
                disabled={isLoading}
              />
              {error && <div className="error">{error}</div>}
            </div>
            <div className="btn-form-group">
              <button type="submit" className="btn btn-primary mb-3 btn-search" disabled={error !== '' || isLoading }>Search</button>
            </div>
          </div>
        </form>

      :

        <div> 
          {data !== null && data !== '' ? 
            <div className="response-container">
              <div>
                  <p>Results:</p>
                  <ul className="result-reg">
                    {data.map((item, index) => (
                      <li key={index}>{item}</li>
                    ))}
                  </ul>
              </div>
              <button type="button" className="btn btn-primary mb-3 btn-frm" onClick={resetForm}>Search again</button>
            </div>
              
            : 
              
            <div className="response-container"> 
              <h3 className="error"> Reg. Number not found. </h3> 
              <button type="button" className="btn btn-primary mb-3 btn-frm" onClick={resetForm}>Try again</button>
            </div>
          }
        </div>
      }
    </div>
  );
};

export default HomeForm;