import { Routes as Switch, Route } from 'react-router-dom';
import HomePage from '../pages/Home/index';

const Routes = () => {
    return (
      <Switch>
        <Route exact path="/" Component={ HomePage }/>
        <Route exact path="home" Component={ HomePage }/>
      </Switch>
    );
  };
  
export default Routes;