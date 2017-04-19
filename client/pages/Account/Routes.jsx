import React from 'react'
import { Route, IndexRoute } from 'react-router'
import { node } from 'prop-types'

import { systemFunction as sF } from 'helpers/checkPermissions'
import ViewPage from './ViewPage'
import Edit from './Edit'

const Layout = props => <div>{props.children}</div>
Layout.propTypes = { children: node.isRequired }

export default (
  <Route path="account" component={Layout}>
    <IndexRoute component={sF('AccountEdit') ? Edit : ViewPage} />
  </Route>
)
