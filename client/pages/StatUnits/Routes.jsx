import React from 'react'
import { Route, IndexRoute } from 'react-router'

import Search from './Search'
import View from './View'
import Edit from './Edit'

const Layout = props => <div>{props.children}</div>

export default (
  <Route path="statunits" component={Layout}>
    <IndexRoute component={Search} />
    <Route path="view/:type/:id" component={View} />
    <Route path="edit/:type/:id" component={Edit} />
  </Route>
)
