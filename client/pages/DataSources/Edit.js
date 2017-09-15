import { connect } from 'react-redux'
import { bindActionCreators } from 'redux'
import { pipe } from 'ramda'
import { lifecycle } from 'recompose'

import withSpinnerUnless from 'components/withSpinnerUnless'
import { getText } from 'helpers/locale'
import { hasValue, hasValues } from 'helpers/schema'
import { edit as actions, clear } from './actions'
import DetailsForm from './DetailsForm'

const { submitData, fetchColumns, fetchDataSource, navigateBack } = actions

const assert = ({ formData, columns }) =>
  hasValue(formData) && hasValue(columns) && hasValues(columns)

const hooks = {
  componentDidMount() {
    this.props.fetchDataSource()
    this.props.fetchColumns()
  },
  componentWillUnmount() {
    this.props.clear()
  },
}

export default pipe(
  withSpinnerUnless(assert),
  lifecycle(hooks),
  connect(
    state => ({
      formData: state.dataSources.editFormData,
      columns: state.dataSources.columns,
      localize: getText(state.locale),
    }),
    (dispatch, props) => bindActionCreators(
      {
        clear,
        navigateBack,
        fetchColumns,
        fetchDataSource: () => fetchDataSource(props.params.id),
        submitData: submitData(props.params.id),
      },
      dispatch,
    ),
  ),
)(DetailsForm)
